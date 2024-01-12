import { defineStore } from 'pinia'
import { get } from './common';
import { type TrackData } from '@/types/types'

export const useTracksStore = defineStore('tracks', {
    state: () => ({
        tracks: [] as TrackData[]
    }),
    actions: {
        async getTracks(query: FormData) {
            const queryUrl = '/tracks?' + new URLSearchParams(query as any)
            this.tracks = await get(queryUrl).then((r) => r.tracks)
        }
    }
})
