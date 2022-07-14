import * as React from "react";
import { QueryClient, QueryClientProvider } from "react-query";

const queryConfig = {
  queries: {
    useErrorBoundary: true,
    refetchOnWindowFocus: false,
    retry(failureCount, error) {
      if (error.status === 404) return false;
      else if (failureCount < 2) return true;
      else return false;
    },
  },
};

const queryClient = new QueryClient();

function AppProviders({ children }) {
  return (
    <QueryClientProvider client={queryClient} config={queryConfig}>
      {children}
    </QueryClientProvider>
  );
}

export { AppProviders, queryClient };
