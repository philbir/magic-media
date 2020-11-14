import apollo from "../apollo";
import MUTATION_ADD_TO_ALBUM from "../graphql/Album/AddItemsToAlbum.gql";

export const addItems = async input => {
  return await apollo.mutate({
    mutation: MUTATION_ADD_TO_ALBUM,
    variables: {
      input: input
    }
  });
};
