import apollo from "../apollo";
import QUERY_ALL_PERSONS from "../graphql/AllPersons.gql";
import QUERY_ALL_GROUPS from "../graphql/AllGroups.gql";
import MUTATION_UPDATE_PERSON from "../graphql/UpdatePersonRequest.gql";
import MUTATION_CREATE_GROUP from "../graphql/CreateGroup.gql";

export const getAllPersons = async () => {
  return await apollo.query({
    query: QUERY_ALL_PERSONS,
    variables: {}
  });
};

export const getAllGroups = async () => {
  return await apollo.query({
    query: QUERY_ALL_GROUPS,
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
        groups: input.groups,
        newGroups: input.newGroups
      }
    }
  });
};

export const createGroup = async name => {
  return await apollo.mutate({
    mutation: MUTATION_CREATE_GROUP,
    variables: {
      name: name
    }
  });
};
