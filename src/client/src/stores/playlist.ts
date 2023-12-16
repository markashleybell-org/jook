import { defineStore } from 'pinia'
import { type TrackListItem } from '@/types/types'

export const usePlaylistStore = defineStore('playlist', {
    state: () => ({
        tracks: [] as TrackListItem[]
    }),
    actions: {
        clear() {
            this.tracks.length = 0
        },
        add(tracks: TrackListItem[]) {
            this.tracks.push(...tracks)
        },
        remove(tracks: TrackListItem[]) {
            const removals = new Set(tracks.map(t => t.trackID))
            this.tracks = this.tracks.filter(t => !removals.has(t.trackID))
        },
        replace(tracks: TrackListItem[]) {
            this.clear()
            this.add(tracks)
        }
    }
})
