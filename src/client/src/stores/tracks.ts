import { defineStore } from 'pinia'
import { type TrackData } from '@/types/types'

// TODO: Move into config
const url = 'https://localhost:5001/tracks'

export const useTracksStore = defineStore('tracks', {
    state: () => ({
        tracks: [] as TrackData[]
    }),
    actions: {
        async getTracks(query: FormData) {
            const queryUrl = url + '?' + new URLSearchParams(query as any)
            this.tracks = await fetch(queryUrl)
                .then((r) => r.json())
                .then((r) => r.tracks)
        }
    }
})
