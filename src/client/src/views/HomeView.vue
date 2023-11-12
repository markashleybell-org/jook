<script setup lang="ts">
import AudioPlayer from '../components/AudioPlayer.vue'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import type { TrackListItem } from '@/types/types'
import { computed, onMounted } from 'vue'

const store = useTracksStore();

const tracks = computed(() => store.tracks.map((t, i) => ({ ...t, index: i })) as TrackListItem[]);

onMounted(() => {
    store.getTracks()
})
</script>

<template>
    <main>
        <AudioPlayer />
        <TrackSearch />
        <TrackList :data="tracks" />
    </main>
</template>
