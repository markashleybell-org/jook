<script setup lang="ts">
import Button from 'primevue/button'
import SelectButton from 'primevue/selectbutton'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import { usePlaylistStore } from '../stores/playlist'
import type { TrackListItem } from '@/types/types'
import { computed, ref } from 'vue'

// TODO: Move into config
const cdn = 'https://jookb2.b-cdn.net'

const tracks = useTracksStore()
const playlist = usePlaylistStore()

const searchResultTracks = computed(() => tracks.tracks.map((t, i) => ({ ...t, index: i })) as TrackListItem[])
const playlistTracks = computed(() => playlist.tracks.map((t) => ({ ...t })) as TrackListItem[])

const playModes = ref([
    { value: 0, label: 'Normal' },
    { value: 1, label: 'Shuffle' }
])

const player = ref<HTMLAudioElement | null>(null)

const playing = ref<boolean>(false)

const nowPlaying = computed(() =>
    playlist.currentTrack
        ? `${playlist.currentTrack.artist} - ${playlist.currentTrack.title}`
        : 'Double-click a track to play'
)

const trackInfo = computed(() =>
    playlist.currentTrack
        ? `Track ${playlist.currentIndex + 1} of ${playlist.tracks.length} ${
              playlist.playMode === 1 ? '(Shuffle)' : ''
          }`
        : ''
)

const nowPlayingSrc = computed(() => (playlist.currentTrack ? cdn + playlist.currentTrack.url : undefined))

async function handleTrackSearchSubmit(query: FormData) {
    await tracks.getTracks(query)
}

function handleTrackSelect(selection: TrackListItem[]) {
    console.table(selection)
}

function handleRemoveButtonClick(track: TrackListItem) {
    playlist.remove([track])
}

function handleAddButtonClick(track: TrackListItem) {
    playlist.add([track])
}

function handleTrackDoubleClick(track: TrackListItem) {
    playlist.add([track])
    handlePlayClick()
}

async function handlePlayClick() {
    if (!playlist.currentTrack) {
        playlist.setTrackIndex(0)
    }
    playing.value = true
}

function handlePauseClick() {
    if (!playlist.currentTrack) {
        playlist.setTrackIndex(0)
    }
    playing.value = false
    player.value?.pause()
}

function handlePreviousClick() {
    playlist.previous()
}

function handleNextClick() {
    playlist.next()
}

async function play() {
    try {
        await player.value?.play()
    } catch (e) {
        console.log(e)
    }
}

function handleEnded() {
    if (playlist.currentIndex >= playlist.tracks.length) {
        return
    }

    playlist.next()
}
</script>

<template>
    <main>
        <audio
            controls
            ref="player"
            :src="nowPlayingSrc"
            preload="metadata"
            @canplay="play"
            @ended="handleEnded"
        ></audio>

        <p>{{ nowPlaying }} &nbsp; {{ trackInfo }}</p>

        <SelectButton
            v-model="playlist.playMode"
            :options="playModes"
            option-label="label"
            option-value="value"
            class=""
        />

        <Button type="button" label="Previous" @click="handlePreviousClick()" icon="pi pi-step-backward" class="mr-2" />

        <Button
            type="button"
            label="Play"
            @click="handlePlayClick()"
            v-show="!playing"
            icon="pi pi-play"
            class="mr-2"
        />

        <Button
            type="button"
            label="Pause"
            @click="handlePauseClick()"
            v-show="playing"
            icon="pi pi-pause"
            class="mr-2"
        />

        <Button type="button" label="Next" @click="handleNextClick()" icon="pi pi-step-forward" class="mr-2" />

        <TrackList
            :height="300"
            :tracks="playlistTracks"
            button-icon="pi pi-minus-circle"
            @track-select="handleTrackSelect"
            @track-button-click="handleRemoveButtonClick"
            @track-double-click="handleTrackDoubleClick"
            v-if="playlist.tracks.length > 0"
        />

        <TrackSearch @submit="handleTrackSearchSubmit" />

        <TrackList
            :height="500"
            :tracks="searchResultTracks"
            button-icon="pi pi-plus-circle"
            @track-select="handleTrackSelect"
            @track-button-click="handleAddButtonClick"
            @track-double-click="handleTrackDoubleClick"
        />
    </main>
</template>
