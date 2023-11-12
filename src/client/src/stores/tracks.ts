import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { type TrackData } from '@/types/types'

const url = 'https://localhost:5001/tracks';

export const useTracksStore = defineStore('tracks', {
    state: () => ({
        tracks: [] as TrackData[]
    }),
    actions: {
        async getTracks(title?: string, artist?: string, album?: string, genre?: string) {
            this.tracks = await fetch(url).then((r) => r.json()).then((r) => r.tracks);
        }
    }
})
