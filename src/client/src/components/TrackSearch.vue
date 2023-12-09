<script setup lang="ts">
import { ref } from 'vue'
import type { TrackSearchQuery } from '@/types/types';

const emit = defineEmits<{
  (e: 'submit', formData: FormData, query: TrackSearchQuery): void
}>()

const query: TrackSearchQuery = {
    title: '',
    artist: 'metronomy',
    album: '',
    genre: ''
};

const searchForm = ref<HTMLFormElement | null>(null);

function handleSubmit() {
    emit('submit', new FormData(searchForm.value!), query);
}
</script>

<template>
    <h1>Search</h1>

    <form method="get" action="/tracks" ref="searchForm" @submit.prevent="handleSubmit">
        <input type="text" name="title" placeholder="Title" v-model="query.title" />
        <input type="text" name="artist" placeholder="Artist" v-model="query.artist" />
        <input type="text" name="album" placeholder="Album" v-model="query.album" />
        <input type="text" name="genre" placeholder="Genre" v-model="query.genre" />
        <button type="submit">Search</button>
    </form>
</template>
