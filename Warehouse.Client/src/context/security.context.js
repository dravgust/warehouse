import { createContext, useReducer, useMemo, useContext, useState, useEffect } from "react";
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
    roles: null,
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

const SupervisorID = "f6694d71d26e40f5a2abb357177c9bdz";
const AdministratorID = "f6694d71d26e40f5a2abb357177c9bdx";

function useUserContext() {
  const [isSupervisor, setIsSupervisor] = useState(false);
  const [isAdministrator, setIsAdministrator] = useState(false);
  const [controller] = useSecurityController();
  const { roles } = controller;
  useEffect(() => {
    if (roles) {
      setIsSupervisor(Boolean(roles.find((role) => role.id === SupervisorID)));
      setIsAdministrator(Boolean(roles.find((role) => role.id === AdministratorID)));
    }
  }, [roles]);

  return { isSupervisor, isAdministrator };
}

export { SecurityControllerProvider, useSecurityController, setRoles, useUserContext };
