<script setup lang="ts">
import type { TrackListItem } from '@/types/types'

import DataTable, { type DataTableRowDoubleClickEvent } from 'primevue/datatable'
import Button from 'primevue/button'
import Column from 'primevue/column'
import { computed, ref, watch } from 'vue'

const props = defineProps<{
    tracks?: TrackListItem[]
    height: number
    buttonIcon: string
}>()

const emit = defineEmits<{
    (e: 'track-select', selection: TrackListItem[]): void
    (e: 'track-button-click', track: TrackListItem): void
    (e: 'track-double-click', track: TrackListItem): void
}>()

const containerHeight = computed(() => `height: ${props.height + 50}px`)
const scrollHeight = computed(() => `${props.height}px`)

const selection = ref<TrackListItem[]>([])

watch(selection, () => {
    /*
    I tried to use the row-select* events on DataTable, but...

    The row-select-all event fires before the selection ref
    is actually updated by the DataTable, so we need to watch
    the ref value and fire *our* selection event whenever that
    happens.

    I thought Vue was supposed to solve exactly this kind of 
    weird timing issue... but here we are again.
    */
    emit('track-select', selection.value)
})

function handleRowButtonClick(track: TrackListItem) {
    emit('track-button-click', track)
}

function handleRowDoubleClick(event: DataTableRowDoubleClickEvent) {
    emit('track-double-click', event.data)
}
</script>

<template>
    <div :style="containerHeight">
        <p>{{ tracks?.length }} items, {{ selection.length }} selected</p>
        <DataTable
            :value="tracks"
            v-model:selection="selection"
            :virtualScrollerOptions="{ itemSize: 40 }"
            :metaKeySelection="false"
            dataKey="trackID"
            @row-dblclick="handleRowDoubleClick"
            tableStyle="cursor: pointer"
            stripedRows
            scrollable
            size="small"
            :scrollHeight="scrollHeight"
            class="mb-4"
        >
            <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>
            <Column field="artist" header="Artist" style="width: 20%"></Column>
            <Column field="album" header="Album" style="width: 30%"></Column>
            <Column field="title" header="Title"></Column>
            <Column class="p-0 text-right">
                <template #body="slotProps">
                    <Button
                        type="button"
                        label=""
                        @click="handleRowButtonClick(slotProps.data)"
                        :icon="buttonIcon"
                        size="small"
                    />
                </template>
            </Column>
        </DataTable>
    </div>
</template>
