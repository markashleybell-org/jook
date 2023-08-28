import mustache from "https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.2.0/mustache.min.js";

import { config } from "./config.js"

const player = document.querySelector("audio");

let currentTrackList = null;
let nowPlayingData = null;

// 0 = sequential, 1 = shuffle
let playMode = 0;
let played = [];

const nowPlayingTemplate = document.getElementById("now-playing-template").innerText;

const playModeSelector = document.querySelector("#play-mode-selector");
const nowPlaying = document.querySelector("#now-playing");

function setNowPlaying(trackInfo) {
    nowPlayingData = trackInfo;

    nowPlaying.innerHTML = mustache.render(nowPlayingTemplate, nowPlayingData);

    if ("mediaSession" in navigator) {
        navigator.mediaSession.metadata = new MediaMetadata({
            title: trackInfo.title,
            artist: trackInfo.artist,
            album: trackInfo.album,
            artwork: [
                {
                    src: "https://dummyimage.com/96x96",
                    sizes: "96x96",
                    type: "image/png",
                },
                {
                    src: "https://dummyimage.com/128x128",
                    sizes: "128x128",
                    type: "image/png",
                },
                {
                    src: "https://dummyimage.com/192x192",
                    sizes: "192x192",
                    type: "image/png",
                },
                {
                    src: "https://dummyimage.com/256x256",
                    sizes: "256x256",
                    type: "image/png",
                },
                {
                    src: "https://dummyimage.com/384x384",
                    sizes: "384x384",
                    type: "image/png",
                },
                {
                    src: "https://dummyimage.com/512x512",
                    sizes: "512x512",
                    type: "image/png",
                },
            ],
        });
    }
}

async function downloadFile(url) {
    const response = await fetch(url);
    const contentLength = response.headers.get("Content-Length");
    const total = parseInt(contentLength, 10);

    let loaded = 0;

    const stream = response.body;
    const reader = stream.getReader();

    return new Response(
        new ReadableStream({
            async start(controller) {
                while (true) {
                    const { done, value } = await reader.read();

                    if (done) {
                        controller.close();
                        break;
                    }

                    loaded += value.byteLength;

                    const progress = Math.round((loaded / total) * 100);

                    console.log(`Downloaded ${progress}%`);

                    controller.enqueue(value);
                }
            },
        })
    );
}

async function deleteFileFromLocalCache(url) {
    const cache = await caches.open("jook-audio");

    return await cache.delete(url);
}

const trackListEntryTemplate = document.getElementById("track-list-entry-template").innerText;

const trackList = document.querySelector("#track-list tbody");

async function startTrack(audioElement, cdn, track) {
    if (track.trackID === nowPlayingData?.trackID)
    {
        audioElement.paused ? await audioElement.play() : await audioElement.pause();
        
        return;
    }

    const cached = await caches.match(track.url).then(r => r ? r.blob() : undefined);

    audioElement.src = cached 
        ? window.URL.createObjectURL(cached)
        : cdn + track.url;

    await audioElement.play();

    setNowPlaying(track);
}

async function start(i) {
    const track = currentTrackList.tracks[i];
    await startTrack(player, config.CDN, track);
}

function playPause() {
    player.paused ? player.play() : player.pause();
}

const searchForm = document.getElementById("search-form");

searchForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const url = searchForm.action + "?" + new URLSearchParams(new FormData(searchForm));
    currentTrackList = await fetch(url).then((r) => r.json());
    trackList.innerHTML = mustache.render(trackListEntryTemplate, currentTrackList);
    played = [];
});

trackList.addEventListener("click", async (e) => {
    if (e.target.nodeName === "TD") {
        const id = e.target.parentNode.dataset.trackid;

        const track = currentTrackList.tracks.find((t) => t.trackID == id);

        await startTrack(player, config.CDN, track);
    }

    if (e.target.classList.contains("download")) {
        e.preventDefault();
        const url = e.target.dataset.url;
        const response = await downloadFile(config.CDN + url);
        const blob = await response.blob();

        const fullResponse = new Response(blob, {
            status: 200,
            headers: { "Content-Type": "audio/mpeg", "Content-Length": blob.size },
        });

        const cache = await caches.open("jook-audio");

        await cache.put(url, fullResponse);
    }
});

player.addEventListener('ended', async (e) => {
    const currentTrackID = nowPlayingData?.trackID;

    if (!currentTrackID) {
        return;
    }

    played.push(currentTrackID);

    const currentTrackIndex = currentTrackList.tracks.findIndex(t => t.trackID == currentTrackID);

    let currentUnplayedIndexes = currentTrackList.tracks
        .map((t,i)=> ({ i: i, trackID: t.trackID }))
        .filter(t => !played.includes(t.trackID))
        .map(t => t.i);
        
    // console.log(currentUnplayedIndexes);

    const nextTrackIndex = playMode === 1
        ? currentUnplayedIndexes[Math.floor(Math.random() * currentUnplayedIndexes.length)] || currentTrackList.tracks.length + 1
        : currentTrackIndex + 1;

    if (nextTrackIndex >= currentTrackList.tracks.length)
    {
        return;
    } 

    await start(nextTrackIndex);
});

playModeSelector.addEventListener('click', async (e) => {
    if (e.target.classList.contains('btn-check')) {
        Array.from(e.target.parentNode.querySelectorAll('.btn-check')).forEach(e => { e.checked = false; });
        e.target.checked = true;
        playMode = parseInt(e.target.dataset.playMode, 10);
    }
});

window.PLAYER = {
    config: config,
    getNowPlayingData: function() { return nowPlayingData },
    start: start,
    playPause: playPause,
    download: downloadFile,
    removeCached: deleteFileFromLocalCache
};
