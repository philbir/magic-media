import apollo from "../apollo";
import QUERY_ALL_PERSONS from "../graphql/AllPersons.gql";
import MUTATION_UPDATE_PERSON from "../graphql/UpdatePersonRequest.gql";

export const getAllPersons = async () => {
  return await apollo.query({
    query: QUERY_ALL_PERSONS,
    variables: {}
  });
};

export const updatePerson = async input => {
  return await apollo.mutate({
    mutation: MUTATION_UPDATE_PERSON,
    variables: {
      input: {
        id: input.id,
        name: input.name,
        dateOfBirth: input.dateOfBirth,
        groups: input.groups
      }
    }
  });
};
