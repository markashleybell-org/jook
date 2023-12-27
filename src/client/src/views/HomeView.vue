<script setup lang="ts">
import AudioPlayer from '../components/AudioPlayer.vue'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import { usePlaylistStore } from '../stores/playlist'
import type { TrackData, TrackListItem, TrackSearchQuery } from '@/types/types'
import { computed, ref } from 'vue'

const trackStore = useTracksStore()
const playlistStore = usePlaylistStore()

const tracks = computed(() => trackStore.tracks.map((t, i) => ({ ...t, index: i })) as TrackListItem[])
const playlist = computed(() => playlistStore.tracks.map((t, i) => ({ ...t })) as TrackListItem[])

const currentTrack = ref<TrackData | null>(null)

async function handleTrackSearchSubmit(query: FormData, x: TrackSearchQuery) {
    await trackStore.getTracks(query)
    console.log(x)
}

function handleTrackSelect(selection: TrackListItem[]) {
    console.table(selection)
}

function handleRemoveButtonClick(track: TrackListItem) {
    playlistStore.remove([track])
}

function handleAddButtonClick(track: TrackListItem) {
    playlistStore.add([track])
}

function handleTrackDoubleClick(track: TrackListItem) {
    currentTrack.value = track
}
</script>

<template>
    <main>
        <AudioPlayer :track="currentTrack" />
        <TrackList
            height="200px"
            :tracks="playlist"
            button-icon="pi pi-minus-circle"
            @track-select="handleTrackSelect"
            @track-button-click="handleRemoveButtonClick"
            @track-double-click="handleTrackDoubleClick"
        />
        <TrackSearch @submit="handleTrackSearchSubmit" />
        <TrackList
            height="600px"
            :tracks="tracks"
            button-icon="pi pi-plus-circle"
            @track-select="handleTrackSelect"
            @track-button-click="handleAddButtonClick"
            @track-double-click="handleTrackDoubleClick"
        />
    </main>
</template>
