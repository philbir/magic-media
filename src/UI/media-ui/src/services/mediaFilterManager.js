/* eslint-disable no-debugger */
import Vue from "vue";

class MediaFilterManager {
    constructor() {
        this.definitions = {
            folder: {
                name: "Folder",
                default: null,
                valueText: (rootState) => rootState.media.filter.folder
            },
            persons: {
                name: "Persons",
                default: [],
                stringValue: (state) => state.filter.persons.join(','),
                valueText: (rootState) => rootState.person.persons.filter(x => rootState.media.filter.persons.includes(x.id))
                    .map(x => x.name)
                    .join(' | ')
            },
            groups: {
                name: "Groups",
                default: [],
                stringValue: (state) => state.filter.groups.join(','),
                valueText: (rootState) => rootState.person.groups.filter(x => rootState.media.filter.groups.includes(x.id))
                    .map(x => x.name)
                    .join(' | ')
            },
            countries: {
                name: "Country",
                default: [],
                stringValue: (state) => state.filter.countries.join(','),
                valueText: (rootState) => {
                    return rootState.media.facets.country.filter(x =>
                        rootState.media.filter.countries.includes(x.value))
                        .map(x => x.text)
                        .join(' | ')
                }
            },
            cities: {
                name: "City",
                default: [],
                stringValue: (state) => state.filter.cities.join(','),
                valueText: (rootState) => rootState.media.facets.city.filter(x =>
                    rootState.media.filter.cities.includes(x.value))
                    .map(x => x.text)
                    .join(' | ')
            },
            cameras: {
                name: "Camera",
                default: [],
                stringValue: (state) => state.filter.cameras.join(','),
                valueText: (rootState) => rootState.media.facets.camera.filter(x =>
                    rootState.media.filter.cameras.includes(x.value))
                    .map(x => x.text)
                    .join(' | ')
            },
            tags: {
                name: "Tags",
                default: [],
                stringValue: (state) => state.filter.tags.join(','),
                valueText: (rootState) => rootState.media.facets.aiTags.filter(x =>
                    rootState.media.filter.tags.includes(x.value))
                    .map(x => x.text)
                    .join(' | ')
            },
            objects: {
                name: "Objects",
                default: [],
                stringValue: (state) => state.filter.objects.join(','),
                valueText: (rootState) => rootState.media.facets.aiTags.filter(x =>
                    rootState.media.filter.objects.includes(x.value))
                    .map(x => x.text)
                    .join(' | ')
            },
            albumId: {
                name: "Album",
                default: null,
                valueText: (rootState) => {
                    if (rootState.media.filter.albumId) {
                        return rootState.album.allAlbums.filter(x =>
                            x.id === rootState.media.filter.albumId)[0].title
                    }
                    return null;
                }
            },
            mediaTypes: {
                name: "Media type",
                default: [],
                valueText: (rootState) => rootState.media.filter.mediaTypes.join(' | ')
            },
            date: {
                name: "Date",
                default: null,
                valueText: (rootState) => rootState.media.filter.date
            },
            geoRadius: {
                name: "Geo",
                default: null,
                stringValue: (state) => {
                    const radius = state.filter.geoRadius;
                    return `${radius.distance}/${radius.latitude.toPrecision},${radius.longitude}`
                },
                valueText: (rootState) => {
                    const radius = rootState.media.filter.geoRadius;
                    return `${radius.distance} km arround ${radius.latitude.toPrecision(2)}, ${radius.longitude.toPrecision(2)}`
                }
            },
        }
    }
    setFilter = (state, key, value) => {
        Vue.set(state.filter, key, value);
    }

    removeFilter = (state, key) => {
        Vue.set(state.filter, key, this.definitions[key].default);
    }
    getDesc = (key, rootState) => {

        var value = rootState.media.filter[key];
        const def = this.definitions[key];

        if (def.stringValue) {
            value = def.stringValue(rootState.media)
        }

        return {
            key,
            name: def.name,
            value: rootState.media.filter[key],
            stringValue: value,
            description: def.valueText(rootState)
        }
    }

    buildDescriptions = (rootState, state) => {
        const filter = state.filter;
        const filterDescs = [];

        for (const key in this.definitions) {

            if (Object.prototype.hasOwnProperty.call(this.definitions, key)) {
                const defaultValue = this.definitions[key].default;

                if (Array.isArray(defaultValue)) {
                    if (filter[key].length > 0) {
                        filterDescs.push(this.getDesc(key, rootState))
                    }
                }
                else {

                    if (filter[key] !== defaultValue) {
                        filterDescs.push(this.getDesc(key, rootState))
                    }
                }
            }
        }

        return filterDescs;
    }
}

export default MediaFilterManager;