/* eslint-disable no-debugger */

class FilterManager {
    /*
      state.filter.persons = [];
      state.filter.mediaTypes = [];
      state.filter.cameras = [];
      state.filter.albumId = null;
      state.filter.geoRadius = null;
      state.filter.folder = null;
      state.filter.date = null;
 
    */
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
                valueText: (rootState) => rootState.person.persons.filter(x => rootState.media.filter.persons.includes(x.id))
                    .map(x => x.name)
                    .join(' | ')
            },
            countries: {
                name: "Country",
                default: [],
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
                valueText: (rootState) => rootState.media.facets.city.filter(x =>
                    rootState.media.filter.cities.includes(x.value))
                    .map(x => x.text)
                    .join(' | ')
            }

        }
    }

    setFilter = (state, key, value) => {
        state.filter[key] = value;
    }

    removeFilter = (state, key) => {
        console.log('REM', key)
        state.filter[key] = this.definitions[key].default;
    }

    getDesc = (key, rootState) => {
        return {
            key,
            name: this.definitions[key].name,
            value: rootState.media.filter[key],
            desc: this.definitions[key].valueText(rootState)
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


export default FilterManager;