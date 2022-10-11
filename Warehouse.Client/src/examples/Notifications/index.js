import { useEffect, useState, forwardRef } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { STREAM_SERVER } from "config/constant";
import Snackbar from "@mui/material/Snackbar";
import MuiAlert from "@mui/material/Alert";

const Alert = forwardRef(function Alert(props, ref) {
  return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

const NotificationBar = () => {
  const [connection, setConnection] = useState(null);
  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${STREAM_SERVER}/stream/notifications`)
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
              setNotification({ open: true, message: item.timestamp });
              console.log("item", item);
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
