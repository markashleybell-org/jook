<script setup lang="ts">
import AudioPlayer from '../components/AudioPlayer.vue'
import TrackSearch from '../components/TrackSearch.vue'
import TrackList from '../components/TrackList.vue'
import { useTracksStore } from '../stores/tracks'
import type { TrackData, TrackListItem } from '@/types/types'
import { computed, ref } from 'vue'
import type { DataTableRowDoubleClickEvent } from 'primevue/datatable'

const store = useTracksStore();

const tracks = computed(() => store.tracks.map((t, i) => ({ ...t, index: i })) as TrackListItem[]);

const currentTrack = ref<TrackData | null>(null);

async function temp(query: FormData) {
    await store.getTracks(query);
}

function tst(event: DataTableRowDoubleClickEvent) {
    currentTrack.value = event.data;
}
</script>

<template>
    <main>
        <AudioPlayer :track="currentTrack" />
        <TrackSearch @submit="temp"/>
        <TrackList :tracks="tracks" @track-double-click="tst" />
    </main>
</template>
