import Vue from 'vue'
import VueApollo from 'vue-apollo'
import { createApolloClient, restartWebsockets } from 'vue-cli-plugin-apollo/graphql-client'

import { setContext } from 'apollo-link-context'

import { WebSocketLink } from 'apollo-link-ws';
import { SubscriptionClient } from 'subscriptions-transport-ws';

import { ApolloLink, split, concat } from 'apollo-link';

import { HttpLink } from 'apollo-link-http';
import { getMainDefinition } from 'apollo-utilities';


// Install the vue plugin
Vue.use(VueApollo)

// Name of the localStorage item
// const AUTH_TOKEN = 'apollo-token'

// Http endpoint
const httpEndpoint = process.env.VUE_APP_GRAPHQL_HTTP || 'http://localhost:5000/graphql'
// Files URL root
export const filesRoot = process.env.VUE_APP_FILES_ROOT || httpEndpoint.substr(0, httpEndpoint.indexOf('/graphql'))

Vue.prototype.$filesRoot = filesRoot

export function createProvider (options = {}) {
const authLink = setContext(async (_, { headers }) => {
  
  let token = window.keycloak.token;

  return {
    headers : {
      ...headers,
      authorization: `Bearer ${token}`
    }
  }
});

// const wsClient = new SubscriptionClient('ws://localhost:5000/graphql', {
//   reconnect: true,
//   connectionParams: () => {
//     return {
//       token: `Bearer ${window.keycloak.token}`
//     }
//   },
//   reconnectionAttempts: 5,
//   lazy: true,
//   connectionCallback: err => {
//     if (err) {
//       console.log('Error Connecting to Subscriptions Server', err);
//     }
//   }
// });

//const wsLink = new WebSocketLink(wsClient);
const wsLink = new WebSocketLink({
  uri: 'ws://localhost:5000/graphql',
  options: {
    reconnect: true,
    reconnectionAttempts: 5,    
    wsOptionArguments: 
    [
      'test123'
    ],
    // connectionParams: () => {
    //   const token = 'test';
    //   console.log('set ws token');
    //   if (!! window.keycloak) {        
    //     return {
    //       authToken: window.keycloak.token
    //     }
    //   } else {
    //     return {
    //       authToken: ''/*'Bearer ' +  window.keycloak.token*/
    //     }
    //   }
    // }
  }
});

const httpLink = new HttpLink({
  uri: httpEndpoint
});

const splitLinks = split(
  ({query}) => {
    const definition = getMainDefinition(query);
    return (
      definition.kind === 'OperationDefinition' && definition.operation === 'subscription'
    );
  },
  wsLink,
  httpLink
);

const link = concat(authLink, splitLinks);
// Config
const defaultOptions = {
  // You can use `https` for secure connection (recommended in production)
  //httpEndpoint,
  // You can use `wss` for secure connection (recommended in production)
  // Use `null` to disable subscriptions
  // wsEndpoint: process.env.VUE_APP_GRAPHQL_WS || 'ws://localhost:5000/graphql',
  // LocalStorage token
  // tokenName: AUTH_TOKEN,
  // Enable Automatic Query persisting with Apollo Engine
  persisting: false,
  // Use websockets for everything (no HTTP)
  // You need to pass a `wsEndpoint` for this to work
  websocketsOnly: false,
  // Is being rendered on the server?
  ssr: false,

  // Override default apollo link
  // note: don't override httpLink here, specify httpLink options in the
  // httpLinkOptions property of defaultOptions.
  link: link

  // Override default cache
  // cache: myCache


  // Additional ApolloClient options
  // apollo: { ... }

  // Client local data (see apollo-link-state)
  // clientState: { resolvers: { ... }, defaults: { ... } }
}

// Call this in the Vue app file

  // Create apollo client
  const client = createApolloClient({
    ...defaultOptions,
    ...options,
  });
  const { apolloClient, wsClient } = client;
  
  console.log(client);

  apolloClient.wsClient = wsLink;

  // Create vue apollo provider
  const apolloProvider = new VueApollo({
    defaultClient: apolloClient,
    defaultOptions: {
      $query: {
        // fetchPolicy: 'cache-and-network',
      },
    },
    errorHandler (error) {
      // eslint-disable-next-line no-console
      console.log('%cError', 'background: red; color: white; padding: 2px 4px; border-radius: 3px; font-weight: bold;', error.message)
    },
  })

  return apolloProvider
}

export async function onLogin(apolloClient, token) {
  console.log(apolloClient.wsClient.subscriptionClient)
  let subscriptionClient = apolloClient.wsClient.subscriptionClient;
  if (subscriptionClient) {
    restartWebsockets(apolloClient.wsClient.subscriptionClient)
  }
  try {
    await apolloClient.resetStore()
  } catch (e) {
    // eslint-disable-next-line no-console
    console.log('%cError on cache reset (login)', 'color: orange;', e.message)
  }  
}



// // Manually call this when user log in
// export async function onLogin (apolloClient, token) {
//   if (typeof localStorage !== 'undefined' && token) {
//     // localStorage.setItem(AUTH_TOKEN, token)
//   }
//   if (apolloClient.wsClient) restartWebsockets(apolloClient.wsClient)
//   try {
//     await apolloClient.resetStore()
//   } catch (e) {
//     // eslint-disable-next-line no-console
//     console.log('%cError on cache reset (login)', 'color: orange;', e.message)
//   }
// }

// // Manually call this when user log out
// export async function onLogout (apolloClient) {
//   if (typeof localStorage !== 'undefined') {
//     // localStorage.removeItem(AUTH_TOKEN)
//   }
//   if (apolloClient.wsClient) restartWebsockets(apolloClient.wsClient)
//   try {
//     await apolloClient.resetStore()
//   } catch (e) {
//     // eslint-disable-next-line no-console
//     console.log('%cError on cache reset (logout)', 'color: orange;', e.message)
//   }
// }
