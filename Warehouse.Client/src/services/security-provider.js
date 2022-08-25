import { useSecurityController, setRoles } from "../context/security.context";
import { useClient } from "../context/auth.context";
import { useState, useEffect } from "react";

export const SecurityPermissions = {
  None: 0,
  View: 1,
  Add: 2,
  Edit: 4,
  Delete: 8,
  Execute: 16,
  Grant: 32,
};

const hasSecurityPermissions = (userPermissions, securityPermissions) =>
  (userPermissions & securityPermissions) === securityPermissions;

//const localStorageKey = "__sec_provider_roles__";
let base = "security";
const useSecurity = (objectName, permissions) => {
  const [state, dispatch] = useSecurityController();
  const [hasPermissions, setHasPermissions] = useState(false);
  const client = useClient();

  useEffect(() => {
    isUserHasPermissions(objectName, permissions).then(setHasPermissions);
  }, []);

  function handleSecurityResponse({ items }) {
    //window.localStorage.setItem(localStorageKey, roles);
    setRoles(dispatch, items);
    return items;
  }
  const fetchRoles = async () => {
    return await client(`${base}/user-roles`, {}).then(handleSecurityResponse);
  };

  const isUserHasPermissions = async (objectName, permissions) => {
    let roles = state.roles;
    if (!roles) {
      roles = await fetchRoles();
    }
    const result = roles.map((role) => {
      const object = role.items.filter((r) => r.objectName === objectName)[0];
      return object && hasSecurityPermissions(object.permissions, permissions);
    });
    return result.indexOf(true) > -1;
  };

  return { hasPermissions };
};
export default useSecurity;
