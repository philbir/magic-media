import apollo from "../apollo";
import MUTATION_ADD_TO_ALBUM from "../graphql/Album/AddItemsToAlbum.gql";
import MUTATION_UPDATE_ALBUM from "../graphql/Album/UpdateAlbum.gql";
import QUERY_ALL_ALBUMS from "../graphql/Album/GetAll.gql";
import QUERY_SEARCH from "../graphql/Album/Search.gql";
import QUERY_GET_ALBUM_MEDIA from "../graphql/Album/GetAlbumMedia.gql";

export const addItems = async input => {
  return await apollo.mutate({
    mutation: MUTATION_ADD_TO_ALBUM,
    variables: {
      input: input
    }
  });
};

export const updateAlbum = async input => {
  return await apollo.mutate({
    mutation: MUTATION_UPDATE_ALBUM,
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

export const getAlbumMedia = async (albumId) => {
  return await apollo.query({
    query: QUERY_GET_ALBUM_MEDIA,
    variables: {
      albumId
    }
  });
};

