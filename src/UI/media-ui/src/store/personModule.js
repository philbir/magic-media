import Vue from 'vue';
import { getAllPersons } from "../services/personService";

const personModule = {
    namespaced: true,
    state: () => ({
        persons: [],

    }),
    mutations: {
        PERSONS_LOADED(state, persons) {
            Vue.set(state, "persons", [...persons]);
        },
        PERSON_ADDED(state, person) {
            var persons = state.persons.filter(x => x.id == person.id);
            if (persons.length === 0) state.persons.push(person);
        }
    },
    actions: {
        async getAll({ commit }) {
            try {
                const res = await getAllPersons();
                commit("PERSONS_LOADED", res.data.persons);
            } catch (ex) {
                console.error(ex);
            }
        }
    },
    getters: {

    },
}

export default personModule;
