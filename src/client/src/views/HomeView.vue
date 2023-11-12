<script setup lang="ts">
import AudioPlayer from '../components/AudioPlayer.vue'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import type { TrackListItem } from '@/types/types'

const store = useTracksStore();

store.fetchData();

const tracks: TrackListItem[] = store.tracks.map((t, i) => ({ ...t, index: i }))
</script>

<template>
    <main>
        <AudioPlayer />

        <TrackSearch />

        <TrackList :data="tracks" />
    </main>
</template>
