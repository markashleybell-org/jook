<script setup lang="ts">
import AudioPlayer from '../components/AudioPlayer.vue'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import type { TrackData, TrackListItem, TrackSearchQuery } from '@/types/types'
import { computed, ref } from 'vue'
import type { DataTableRowDoubleClickEvent } from 'primevue/datatable'

const store = useTracksStore()

const tracks = computed(() => store.tracks.map((t, i) => ({ ...t, index: i })) as TrackListItem[])

const currentTrack = ref<TrackData | null>(null)

async function handleTrackSearchSubmit(query: FormData, x: TrackSearchQuery) {
    await store.getTracks(query)
    console.log(x)
}

function handleTrackDoubleClick(event: DataTableRowDoubleClickEvent) {
    currentTrack.value = event.data
}
</script>

<template>
    <main>
        <AudioPlayer :track="currentTrack" />
        <TrackSearch @submit="handleTrackSearchSubmit" />
        <TrackList :tracks="tracks" @track-double-click="handleTrackDoubleClick" />
    </main>
</template>
