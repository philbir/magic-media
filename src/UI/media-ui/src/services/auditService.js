import apollo from "../apollo";
import QUERY_SEARCH from "../graphql/Audit/Search.gql"

export const search = async (request) => {
    return await apollo.query({
        query: QUERY_SEARCH,
        variables: { request }
    });
};
