<script setup lang="ts">
import TrackSearchInput from '../components/TrackSearchInput.vue'
import { ref } from 'vue'
import type { TrackSearchQuery } from '@/types/types'

const emit = defineEmits<{
    (e: 'submit', formData: FormData, query: TrackSearchQuery): void
}>()

const query: TrackSearchQuery = {
    title: '',
    artist: 'metronomy',
    album: '',
    genre: ''
}

const searchForm = ref<HTMLFormElement | null>(null)

function handleSubmit() {
    emit('submit', new FormData(searchForm.value!), query)
}
</script>

<template>
    <form
        method="get"
        action="/tracks"
        class="formgrid grid"
        ref="searchForm"
        @submit.prevent="handleSubmit"
    >
        <TrackSearchInput name="title" label="Title" v-model="query.title" />
        <TrackSearchInput name="artist" label="Artist" v-model="query.artist" />
        <TrackSearchInput name="album" label="Album" v-model="query.album" />
        <TrackSearchInput name="genre" label="Genre" v-model="query.genre" />
        <div class="field col relative">
            <button
                type="submit"
                class="absolute bottom-0 bg-primary border-primary-500 px-3 py-2 text-base border-1 border-solid border-round cursor-pointer transition-all transition-duration-200 hover:bg-primary-600 hover:border-primary-600 active:bg-primary-700 active:border-primary-700"
            >
                Search
            </button>
        </div>
    </form>
</template>
