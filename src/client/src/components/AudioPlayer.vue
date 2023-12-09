<script setup lang="ts">
import type { TrackData } from '@/types/types'
import { computed, ref } from 'vue';

const props = defineProps<{
    track?: TrackData | null
}>()

const cdn = 'https://jookb2.b-cdn.net';

const src = computed(() => props.track ? cdn + props.track.url : undefined);

const player = ref<HTMLAudioElement | null>(null);
</script>

<template>
    <span>{{track?.title}}</span>
    <audio controls ref="player" :src="src" @canplay="player?.play()"></audio>
</template>
