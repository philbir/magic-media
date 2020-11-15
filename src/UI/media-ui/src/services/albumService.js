import apollo from "../apollo";
import MUTATION_ADD_TO_ALBUM from "../graphql/Album/AddItemsToAlbum.gql";
import QUERY_ALL_ALBUMS from "../graphql/Album/GetAll.gql";
import QUERY_SEARCH from "../graphql/Album/Search.gql";

export const addItems = async input => {
  return await apollo.mutate({
    mutation: MUTATION_ADD_TO_ALBUM,
    variables: {
      input: input
    }
  });
};

export const getAllAlbums = async () => {
  return await apollo.query({
    query: QUERY_ALL_ALBUMS,
    variables: {}
  });
};

export const searchAlbums = async (input) => {
  return await apollo.query({
    query: QUERY_SEARCH,
    variables: {
      input
    }
  });
};