import { useEffect, useState, forwardRef } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { STREAM_SERVER } from "config/constant";
import Snackbar from "@mui/material/Snackbar";
import MuiAlert from "@mui/material/Alert";
import * as auth from "services/auth-provider";

const STREAM_URL = process.env.REACT_APP_STREAM_URL || STREAM_SERVER;

const Alert = forwardRef(function Alert(props, ref) {
  return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

const NotificationBar = () => {
  const [connection, setConnection] = useState(null);
  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${STREAM_URL}/stream/notifications`, {
        withCredentials: false,
        accessTokenFactory: () => auth.getToken() ?? Promise.reject("No user is logged in."),
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  const [notification, setNotification] = useState({
    open: false,
    message: "",
  });

  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }
    setNotification({ ...notification, open: false });
  };

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log("Connected!");
          connection.stream("Notifications").subscribe({
            next: (item) => {
              console.log("item", item);
              let message = "";
              switch (item) {
                default:
                  message = item.message;
              }
              setNotification({ open: true, message: message });
            },
            complete: () => {
              console.log("completed");
            },
            error: (err) => {
              console.log("error", err);
            },
          });
        })
        .catch((e) => console.log("Connection failed: ", e));
    }
  }, [connection]);
  return (
    <Snackbar
      open={notification.open}
      autoHideDuration={5000}
      onClose={handleClose}
      anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
    >
      <Alert onClose={handleClose} severity="success" sx={{ width: "100%" }}>
        {notification.message}
      </Alert>
    </Snackbar>
  );
};
export default NotificationBar;
