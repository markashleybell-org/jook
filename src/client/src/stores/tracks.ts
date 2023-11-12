import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { type TrackData } from '@/types/types'

export const useTracksStore = defineStore('tracks', {
    state: () => ({
        tracks: [] as TrackData[]
    }),
    actions: {
        fetchData() {
            this.tracks = [
                {
                    trackID: 1234,
                    trackNumber: 1,
                    title: 'TEST 1',
                    url: '/test1',
                    albumArtistID: 100,
                    albumArtist: 'TEST',
                    albumID: 200,
                    album: 'TEST',
                    artistID: 300,
                    artist: 'TEST'
                }
            ]
        }   
    }
})
