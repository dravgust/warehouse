import * as React from "react";
import { QueryClient, QueryClientProvider } from "react-query";
import { ReactQueryDevtools } from "react-query/devtools";
import PropTypes from "prop-types";

const queryConfig = {
  defaultOptions: {
    queries: {
      retry: (failureCount, error) => {
        if (error.status === 404) return false;
        else if (failureCount < 2) return true;
        else return false;
      },
      refetchOnMount: "always",
      refetchOnWindowFocus: false,
      refetchOnReconnect: "always",
      //cacheTime: 1000 * 30, //30 seconds
      //refetchInterval: 1000 * 30, //30 seconds
      //refetchIntervalInBackground: false,
      suspense: false,
      //staleTime: 1000 * 30,
    },
    mutations: {
      retry: 2,
    },
  },
};

const queryClient = new QueryClient(queryConfig);

function AppProviders({ children }) {
  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export { AppProviders, queryClient };

AppProviders.propTypes = {
  children: PropTypes.node.isRequired,
};
