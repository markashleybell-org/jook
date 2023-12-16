<script setup lang="ts">
import type { TrackListItem } from '@/types/types'

import DataTable, { type DataTableRowDoubleClickEvent } from 'primevue/datatable'
import Column from 'primevue/column'
import { ref, watch } from 'vue'

defineProps<{
    tracks?: TrackListItem[]
}>()

const emit = defineEmits<{
    (e: 'track-select', selection: TrackListItem[]): void
    (e: 'track-double-click', track: TrackListItem): void
}>()

const selection = ref<TrackListItem[]>([])

watch(selection, () => {
    /*
    I tried to use the row-select* events on DataTable, but...

    The row-select-all event fires before the selection ref
    is actually updated by the DataTable, so we need to watch
    the ref value and fire *our* selection event whenever that
    happens.

    I thought Vue was supposed to solve exactly this kind of 
    weird issue... but here we are again.
    */
    emit('track-select', selection.value)
})

function handleRowDoubleClick(event: DataTableRowDoubleClickEvent) {
    emit('track-double-click', event.data)
}
</script>

<template>
    <p>{{ selection.length }} selected</p>
    <DataTable
        :value="tracks"
        v-model:selection="selection"
        :virtualScrollerOptions="{ itemSize: 52 }"
        :metaKeySelection="false"
        dataKey="trackID"
        @row-dblclick="handleRowDoubleClick"
        tableStyle="min-width: 50rem"
        stripedRows
        scrollable
        scrollHeight="600px"
    >
        <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>
        <Column field="artist" header="Artist" style="width: 20%"></Column>
        <Column field="album" header="Album" style="width: 30%"></Column>
        <Column field="title" header="Title"></Column>
    </DataTable>
</template>
