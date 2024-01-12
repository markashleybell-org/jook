import { defineStore } from 'pinia'
import { post } from './common';
import { type TrackListItem } from '@/types/types'
import { toRaw } from 'vue';

function shuffle(array: TrackListItem[]) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1))
        ;[array[i], array[j]] = [array[j], array[i]]
    }
    return array
}

export const usePlaylistStore = defineStore('playlist', {
    state: () => ({
        playMode: 0,
        currentIndex: 0,
        currentTrack: null as TrackListItem | null,
        tracks: [] as TrackListItem[],
        shuffledTracks: [] as TrackListItem[]
    }),
    actions: {
        clear() {
            this.tracks.length = 0
        },
        add(tracks: TrackListItem[]) {
            this.tracks.push(...tracks)
            this.shuffledTracks = shuffle(this.tracks.slice(0))
        },
        remove(tracks: TrackListItem[]) {
            const removals = new Set(tracks.map((t) => t.trackID))
            this.tracks = this.tracks.filter((t) => !removals.has(t.trackID))
            this.shuffledTracks = shuffle(this.tracks.slice(0))
        },
        replace(tracks: TrackListItem[]) {
            this.clear()
            this.add(tracks)
        },
        setTrackIndex(index: number) {
            this.currentIndex = index
            this.currentTrack = this.playMode === 1 ? this.shuffledTracks[index] : this.tracks[index]
        },
        previous() {
            const previousIndex = this.currentIndex - 1

            if (previousIndex < 0) {
                return
            }

            this.setTrackIndex(previousIndex)
        },
        next() {
            const nextIndex = this.currentIndex + 1

            if (nextIndex >= this.tracks.length) {
                return
            }

            this.setTrackIndex(nextIndex)
        },
        async save() {
            const payload: any = {
                tracks: toRaw(this.tracks)
            };

            await post("/playlist", payload)
        }
    }
})
