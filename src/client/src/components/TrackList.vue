<script setup lang="ts">
import type { TrackListItem } from '@/types/types'

import DataTable, { type DataTableRowDoubleClickEvent } from 'primevue/datatable'
import Column from 'primevue/column'

defineProps<{
    tracks?: TrackListItem[]
}>()

const emit = defineEmits<{
  (e: 'track-double-click', event: DataTableRowDoubleClickEvent): void
}>()

function tmp(event: DataTableRowDoubleClickEvent) {
    emit('track-double-click', event);
}
</script>

<template>
    <DataTable 
        :value="tracks" 
        :virtualScrollerOptions="{ itemSize: 52 }"
        @row-dblclick="tmp"
        tableStyle="min-width: 50rem" 
        stripedRows 
        scrollable 
        scrollHeight="600px">
        <Column field="artist" header="Artist" style="width: 20%"></Column>
        <Column field="album" header="Album" style="width: 30%"></Column>
        <Column field="title" header="Title"></Column>
    </DataTable>
</template>
