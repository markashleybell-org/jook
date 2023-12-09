<script setup lang="ts">
import AudioPlayer from '../components/AudioPlayer.vue'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import type { TrackListItem } from '@/types/types'
import { computed } from 'vue'

const store = useTracksStore();

const tracks = computed(() => store.tracks.map((t, i) => ({ ...t, index: i })) as TrackListItem[]);

async function temp(query: FormData) {
    await store.getTracks(query);
}
</script>

<template>
    <main>
        <AudioPlayer />
        <TrackSearch @submit="temp"/>
        <TrackList :data="tracks" />
    </main>
</template>
