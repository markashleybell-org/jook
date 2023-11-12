<script setup lang="ts">
import { type TrackListItem } from '@/types/types'

import DataTable from 'primevue/datatable'
import Column from 'primevue/column'

defineProps<{
    data?: TrackListItem[]
}>()
</script>

<template>
    <DataTable :value="data" tableStyle="min-width: 50rem">
        <Column field="albumArtist" header="Album Artist"></Column>
        <Column field="artist" header="Artist"></Column>
        <Column field="album" header="Album"></Column>
        <Column field="title" header="Title"></Column>
    </DataTable>
</template>
