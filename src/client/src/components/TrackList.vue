<script setup lang="ts">
import { type TrackListItem } from '@/types/types'

import DataTable from 'primevue/datatable'
import Column from 'primevue/column'

defineProps<{
    data?: TrackListItem[]
}>()
</script>

<template>
    <DataTable :value="data" tableStyle="min-width: 50rem" stripedRows scrollable scrollHeight="600px" :virtualScrollerOptions="{ itemSize: 52 }">
        <Column field="artist" header="Artist" style="width: 20%"></Column>
        <Column field="album" header="Album" style="width: 30%"></Column>
        <Column field="title" header="Title"></Column>
    </DataTable>
</template>
