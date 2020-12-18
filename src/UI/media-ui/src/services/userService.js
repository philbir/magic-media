import apollo from "../apollo";
import MUTATION_CREATE_FROM_PERSON from "../graphql/User/CreateFromPerson.gql";
import QUERY_ALL_USERS from "../graphql/User/AllUsers.gql"
import QUERY_ME from "../graphql/User/Me.gql"

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
