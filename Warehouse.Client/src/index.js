import React from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "App";
import { SoftUIControllerProvider } from "context";
import { AuthProvider } from "context/auth.context";
import { AppProviders } from "context/app.context";
import { StoreControllerProvider } from "./context/store.context";
import { SecurityControllerProvider } from "./context/security.context";
import { ResourcesProvider } from "./context/resources.context";

const container = document.getElementById("root");
const root = createRoot(container);

root.render(
  <BrowserRouter>
    <SoftUIControllerProvider>
      <ResourcesProvider>
        <StoreControllerProvider>
          <AppProviders>
            <AuthProvider>
              <SecurityControllerProvider>
                <App />
              </SecurityControllerProvider>
            </AuthProvider>
          </AppProviders>
        </StoreControllerProvider>
      </ResourcesProvider>
    </SoftUIControllerProvider>
  </BrowserRouter>
);
