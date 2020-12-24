import apollo from "../apollo";
import MUTATION_CREATE_FROM_PERSON from "../graphql/User/CreateFromPerson.gql";
import MUTATION_CREATE_INVITE from "../graphql/User/CreateInvite.gql";
import MUTATION_SAVE_SHARED_ALBUMS from "../graphql/User/SaveUserSharedAlbums.gql";
import QUERY_ALL_USERS from "../graphql/User/AllUsers.gql"
import QUERY_ME from "../graphql/User/Me.gql"
import QUERY_BY_ID from "../graphql/User/GetById.gql"
import QUERY_SEARCH from "../graphql/User/Search.gql"

export const createUserFromPerson = async (personId, email) => {
    return await apollo.mutate({
        mutation: MUTATION_CREATE_FROM_PERSON,
        variables: {
            input: {
                personId,
                email
            }
        }
    });
};

export const createInvite = async (id) => {
    return await apollo.mutate({
        mutation: MUTATION_CREATE_INVITE,
        variables: {
            id
        }
    });
};

export const saveSharedAlbums = async (input) => {
    return await apollo.mutate({
        mutation: MUTATION_SAVE_SHARED_ALBUMS,
        variables: {
            input
        }
    });
};


export const getAllUsers = async () => {
    return await apollo.query({
        query: QUERY_ALL_USERS,
        variables: {}
    });
};

export const getMe = async () => {
    return await apollo.query({
        query: QUERY_ME,
        variables: {}
    });
};

export const getById = async (id) => {
    return await apollo.query({
        query: QUERY_BY_ID,
        variables: { id }
    });
};

export const search = async (input) => {
    return await apollo.query({
        query: QUERY_SEARCH,
        variables: { input }
    });
};
