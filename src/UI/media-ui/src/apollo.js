import { ApolloClient } from 'apollo-client';
import { HttpLink } from 'apollo-link-http';
import { InMemoryCache } from 'apollo-cache-inmemory';

const defaultOptions = {
  watchQuery: {
    fetchPolicy: 'network-only',
    errorPolicy: 'ignore',
  },
  query: {
    fetchPolicy: 'network-only',
    errorPolicy: 'all',
  },
}

export default new ApolloClient({
  // Provide the URL to the API server.
  link: new HttpLink({ uri: '/graphql' }),
  // Using a cache for blazingly
  // fast subsequent queries.
  cache: new InMemoryCache({
    resultCaching: false
  }),
  defaultOptions: defaultOptions,
});