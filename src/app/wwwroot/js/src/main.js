import mustache from "mustache"

import { config } from "./config.js"

const player = document.querySelector("audio");

// 0 = sequential, 1 = shuffle
let playMode = 0;

let currentSearchList = [];

let currentPlayListOrdered = [];
let currentPlayListShuffled = [];
let currentPlayListIndex = 0;

let nowPlayingData = null;

const nowPlayingTemplate = document.getElementById("now-playing-template").innerText;
const searchListEntryTemplate = document.getElementById("search-list-entry-template").innerText;
const playListEntryTemplate = document.getElementById("play-list-entry-template").innerText;

const playModeSelector = document.getElementById("play-mode-selector");
const nowPlaying = document.getElementById("now-playing");
const searchForm = document.getElementById("search-form");
const searchList = document.querySelector("#search-list tbody");
const playList = document.querySelector("#play-list tbody");
const clearPlaylist = document.getElementById("clear-playlist");
const replacePlaylist = document.getElementById("replace-playlist");
const playlistPlay = document.getElementById("playlist-play");
const playlistPrevious = document.getElementById("playlist-previous");
const playlistNext = document.getElementById("playlist-next");

function shuffle(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

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

function setCurrentPlayListIndex(index) {
    currentPlayListIndex = index;
    console.log(currentPlayListIndex);
}

function addToPlayList(track) {
    currentPlayListOrdered.push(track);
    currentPlayListShuffled = shuffle(currentPlayListOrdered.slice(0));
}

function removeFromPlayList(index) {
    currentPlayListOrdered.splice(index, 1);
    currentPlayListShuffled = shuffle(currentPlayListOrdered.slice(0));
}

async function startTrack(audioElement, cdn, track) {
    const cached = await caches.match(track.url).then(r => r ? r.blob() : undefined);

    audioElement.src = cached 
        ? window.URL.createObjectURL(cached)
        : cdn + track.url;

    await audioElement.play();

    setNowPlaying(track);

    playList.querySelector('.playing-track')?.classList.remove('playing-track');
    playList.querySelector(`[data-trackid="${track.trackID}"]`)?.classList.add('playing-track');
}

async function start(i) {
    const track = (playMode === 1 ? currentPlayListShuffled : currentPlayListOrdered)[i];
    await startTrack(player, config.CDN, track);
}

function playPause() {
    player.paused ? player.play() : player.pause();
}

function renderTrackList(template, trackList) {
    return mustache.render(template, { 
        tracks: trackList.map((t, i) => ({
            albumArtist: t.albumArtist,
            album: t.album,
            artist: t.artist,
            trackID: t.trackID,
            title: t.title,
            url: t.url,
            index: i
        }))
    });
}

function renderSearchList(trackList) {
    return renderTrackList(searchListEntryTemplate, trackList);
}

function renderPlayList(trackList) {
    return renderTrackList(playListEntryTemplate, trackList);
}

searchForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const url = searchForm.action + "?" + new URLSearchParams(new FormData(searchForm));
    currentSearchList = await fetch(url).then((r) => r.json()).then((r) => r.tracks);
    searchList.innerHTML = renderSearchList(currentSearchList);
});

playList.addEventListener("click", async (e) => {
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

    if (e.target.classList.contains("remove-from-playlist")) {
        e.preventDefault();
        const index = parseInt(e.target.dataset.trackindex, 10);
        removeFromPlayList(index);
        playList.innerHTML = renderPlayList(currentPlayListOrdered);
    }
});

player.addEventListener('ended', async (e) => {
    setCurrentPlayListIndex(currentPlayListIndex + 1);

    if (currentPlayListIndex >= currentPlayListOrdered.length) {
        setCurrentPlayListIndex(0);
        return;
    }

    await start(currentPlayListIndex);
});

playModeSelector.addEventListener('click', async (e) => {
    if (e.target.classList.contains('btn-check')) {
        Array.from(e.target.parentNode.querySelectorAll('.btn-check')).forEach(e => { e.checked = false; });
        e.target.checked = true;
        playMode = parseInt(e.target.dataset.playMode, 10);
    }
});

clearPlaylist.addEventListener('click', async (e) => {
    setCurrentPlayListIndex(0);
    currentPlayListOrdered = [];
    currentPlayListShuffled = [];
    playList.innerHTML = renderPlayList(currentPlayListOrdered);
});

replacePlaylist.addEventListener('click', async (e) => {
    setCurrentPlayListIndex(0);
    currentPlayListOrdered = currentSearchList.slice(0);
    currentPlayListShuffled = shuffle(currentPlayListOrdered.slice(0));
    playList.innerHTML = renderPlayList(currentPlayListOrdered);
});

searchList.addEventListener('click', async (e) => {
    if (e.target.nodeName === "TD") {
        const trackID = parseInt(e.target.parentNode.dataset.trackid, 10);

        if (trackID === nowPlayingData?.trackID)
        {
            playPause();
            return;
        }

        const index = parseInt(e.target.parentNode.dataset.trackindex, 10);

        await startTrack(player, config.CDN, currentSearchList[index]);

        return;
    }

    if (e.target.classList.contains("add-to-playlist")) {
        e.preventDefault();
        const index = parseInt(e.target.dataset.trackindex, 10);
        addToPlayList(currentSearchList[index]);
        playList.innerHTML = renderPlayList(currentPlayListOrdered);
    }
});

playlistPlay.addEventListener('click', async (e) => {
    e.preventDefault();

    if (!currentPlayListOrdered.length)
    {
        return;
    }

    setCurrentPlayListIndex(0);

    await start(currentPlayListIndex);
});

playlistPrevious.addEventListener('click', async (e) => {
    const previousIndex = currentPlayListIndex - 1;

    if (previousIndex < 0)
    {
        return;
    }

    setCurrentPlayListIndex(previousIndex);

    await start(currentPlayListIndex);
});

playlistNext.addEventListener('click', async (e) => {
    const nextIndex = currentPlayListIndex + 1;

    if (nextIndex >= currentPlayListOrdered.length)
    {
        return;
    }

    setCurrentPlayListIndex(nextIndex);

    await start(currentPlayListIndex);
});

window.PLAYER = {
    config: config
};
