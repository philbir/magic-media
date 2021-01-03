import apollo from "../apollo";
import MUTATION_ADD_TO_ALBUM from "../graphql/Album/AddItemsToAlbum.gql";
import MUTATION_REMOVE_FOLDERS from "../graphql/Album/RemoveFoldersFromAlbum.gql";
import MUTATION_UPDATE_ALBUM from "../graphql/Album/UpdateAlbum.gql";
import MUTATION_SET_COVER from "../graphql/Album/SetCover.gql";
import MUTATION_DELETE from "../graphql/Album/Delete.gql";
import QUERY_ALL_ALBUMS from "../graphql/Album/GetAll.gql";
import QUERY_SEARCH from "../graphql/Album/Search.gql";
import QUERY_GET_ALBUM_MEDIA from "../graphql/Album/GetAlbumMedia.gql";
import QUERY_GET_BY_ID from "../graphql/Album/GetAlbumById.gql";

export const addItems = async input => {
  return await apollo.mutate({
    mutation: MUTATION_ADD_TO_ALBUM,
    variables: {
      input: input
    }
  });
};

export const removeFolders = async input => {
  return await apollo.mutate({
    mutation: MUTATION_REMOVE_FOLDERS,
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

export const setCover = async input => {
  return await apollo.mutate({
    mutation: MUTATION_SET_COVER,
    variables: {
      input: input
    }
  });
};


export const deleteAlbum = async id => {
  return await apollo.mutate({
    mutation: MUTATION_DELETE,
    variables: {
      id
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

export const getAlbumById = async (id) => {
  return await apollo.query({
    query: QUERY_GET_BY_ID,
    variables: {
      id
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

