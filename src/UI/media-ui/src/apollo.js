import { InMemoryCache } from "apollo-cache-inmemory";
import { ApolloClient } from "apollo-client";
import { HttpLink } from "apollo-link-http";

const defaultOptions = {
  watchQuery: {
    fetchPolicy: "network-only",
    errorPolicy: "ignore"
  },
  query: {
    fetchPolicy: "network-only",
    errorPolicy: "all"
  }
};

const httpLink = new HttpLink({
  uri: "/graphql"
});

export default new ApolloClient({
  link: httpLink,
  cache: new InMemoryCache({
    resultCaching: false
  }),
  defaultOptions: defaultOptions
});


