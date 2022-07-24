/**
 =========================================================
 * Soft UI Dashboard React - v3.1.0
 =========================================================

 * Product Page: https://www.creative-tim.com/product/soft-ui-dashboard-react
 * Copyright 2022 Creative Tim (https://www.creative-tim.com)

 Coded by www.creative-tim.com

 =========================================================

 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

import React from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "App";

// Soft UI Dashboard React Context Provider
import { SoftUIControllerProvider } from "context";

import { AuthProvider } from "context/auth.context";
import { AppProviders } from "context/app.context";
import { StoreControllerProvider } from "./context/store.context";

const container = document.getElementById("root");
const root = createRoot(container);

root.render(
  <BrowserRouter>
    <SoftUIControllerProvider>
      <StoreControllerProvider>
        <AppProviders>
          <AuthProvider>
            <App />
          </AuthProvider>
        </AppProviders>
      </StoreControllerProvider>
    </SoftUIControllerProvider>
  </BrowserRouter>
);
