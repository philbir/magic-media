import apollo from '../apollo'
import QUERY_SEARCH from "../graphql/SearchMedia.gql";
import QUERY_GETBYID from "../graphql/GetMediaDetails.gql";
import QUERY_SEARCH_FACETS from "../graphql/SearchFacets.gql";

export const searchMedia = async (request) => {
  return await apollo.query({
    query: QUERY_SEARCH,
    variables: {
      request: request
    }
  });
};

export const getById = async (id) => {
  return await apollo.query({
    query: QUERY_GETBYID,
    variables: {
      id: id,
    },
  });
};


export const getSearchFacets = async () => {
  return await apollo.query({
    query: QUERY_SEARCH_FACETS,
    variables: {
    },
  });
};