import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./context/auth.context";
import { useNavigate } from "react-router-dom";

{
  /*export const ProtectedRoute = () => {
    const navigate = useNavigate();
    let { user } = useAuth();

    if (!user || !user.token || user.token === "") {
      return (
        <SweetAlert
          warning
          title="You must be signed in!"
          onCancel={() => navigate("/authentication/sign-in")}
          onConfirm={() => navigate("/authentication/sign-in")}
          confirmBtnBsStyle="primary"
        />
      );
    }
  
    return <Outlet />;
  };
*/
}
export const PrivateRoute = () => {
  let { user } = useAuth();

  if (!user || !user.token || user.token === "") {
    return <Navigate to="/authentication/sign-in" />;
  }
  return <Outlet />;
};
