import { createContext, useReducer, useMemo, useContext } from "react";
import PropTypes from "prop-types";

const SecurityContext = createContext(null);
SecurityContext.displayName = "SecurityContext";

const actionMap = {
  ROLES: (state, action) => ({ ...state, roles: action.value }),
};

function reducer(state, action) {
  const handler = actionMap[action.type];
  return handler ? handler(state, action) : state;
}

function SecurityControllerProvider({ children }) {
  const initialState = {
    roles: [],
  };
  const [controller, dispatch] = useReducer(reducer, initialState);
  const value = useMemo(() => [controller, dispatch], [controller, dispatch]);
  return <SecurityContext.Provider value={value}>{children}</SecurityContext.Provider>;
}

function useSecurityController() {
  const context = useContext(SecurityContext);
  if (!context) {
    throw new Error("useSecurityController should be used inside the SecurityControllerProvider.");
  }
  return context;
}

SecurityControllerProvider.prototype = {
  children: PropTypes.node.isRequired,
};

const setRoles = (dispatch, value) => dispatch({ type: "ROLES", value });

export { SecurityControllerProvider, useSecurityController, setRoles };
