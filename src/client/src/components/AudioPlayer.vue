<script setup lang="ts">
import type { TrackData } from '@/types/types'
import { computed, ref } from 'vue'

// TODO: Move into config
const cdn = 'https://jookb2.b-cdn.net'

const props = defineProps<{
    track?: TrackData | null
}>()

const player = ref<HTMLAudioElement | null>(null)

const src = computed(() => (props.track ? cdn + props.track.url : undefined))

const nowPlaying = computed(() => (props.track ? `${props.track.artist} - ${props.track.title}` : 'Double-click a track to play'))
</script>

<template>
    <audio controls ref="player" :src="src" @canplay="player?.play()"></audio>
    <p>{{ nowPlaying }}</p>
</template>
