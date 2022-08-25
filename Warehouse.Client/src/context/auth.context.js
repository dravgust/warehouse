import React from "react";
import { queryClient } from "context/app.context";
import * as auth from "services/auth-provider";
import { client } from "utils/api-client";
import { useAsync } from "utils/hooks";
import { useResources, setResources } from "./resources.context";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";

async function bootstrapAppData(dispatch) {
  let user = null;
  const token = await auth.getToken();
  if (token) {
    const data = await client("account/bootstrap", { token });
    console.log("bootstrap", data);
    setResources(dispatch, data.resources);
    /*queryCache.setQueryData('list-items', data.listItems, {
      staleTime: 5000,
    })
    for (const listItem of data.listItems) {
      setQueryDataForBook(listItem.book)
    }*/
    user = { ...data.user, token };
  }
  return user;
}

const AuthContext = React.createContext(null);

function AuthProvider(props) {
  const {
    data: user,
    status,
    error,
    isLoading,
    isIdle,
    isError,
    isSuccess,
    run,
    setData,
  } = useAsync();

  const [, dispatch] = useResources();

  React.useEffect(() => {
    const appDataPromise = bootstrapAppData(dispatch);
    run(appDataPromise);
  }, [run]);

  const login = React.useCallback(
    (form) => auth.login(form).then((user) => setData(user)),
    [setData]
  );
  const register = React.useCallback(
    (form) => auth.register(form).then((user) => setData(user)),
    [setData]
  );

  const logout = React.useCallback(() => {
    queryClient.clear();
    auth.logout();
    setData(null);
  }, [setData]);

  const value = React.useMemo(
    () => ({ user, login, logout, register }),
    [login, logout, register, user]
  );

  if (isLoading || isIdle) {
    return (
      <Backdrop sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }} open={true}>
        <CircularProgress color="inherit" />
      </Backdrop>
    );
  }

  if (isError) {
    //return <FullPageErrorFallback error={error} />
    return <div>Error: {error}</div>;
  }

  if (isSuccess) {
    return <AuthContext.Provider value={value} {...props} />;
  }

  throw new Error(`Unhandled status: ${status}`);
}

function useAuth() {
  const context = React.useContext(AuthContext);
  if (context === undefined) {
    throw new Error(`useAuth must be used within a AuthProvider`);
  }
  return context;
}

function useClient() {
  const { user } = useAuth();
  const token = user?.token;
  return React.useCallback((endpoint, config) => client(endpoint, { ...config, token }), [token]);
}

export { AuthProvider, useAuth, useClient };
