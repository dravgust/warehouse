import * as React from "react";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContentText from "@mui/material/DialogContentText";
import { useFormik } from "formik";
import { useMutation } from "react-query";
import * as yup from "yup";
// Soft UI Dashboard React components
import Stack from "@mui/material/Stack";
import { TextField } from "@mui/material";
import * as auth from "auth-provider";
import { client } from "utils/api-client";
import Autocomplete from "@mui/material/Autocomplete";

const validationSchema = yup.object({
  name: yup
    .string("Enter product name")
    .min(3, "Name should be of minimum 3 characters length")
    .required("Name is required"),
  description: yup
    .string("Enter product description")
    .min(8, "Description should be of minimum 8 characters length"),
  macAddress: yup
    .string("Enter MAC address")
    .min(5, "MAC address should be of minimum 6 characters length"),
});

export default function AddProduct({ open, handleClose, editItem }) {
  const saveItem = async (item) => {
    const token = await auth.getToken();
    const res = await client(`items/set`, {
      data: item,
      token,
    });
    return res;
  };

  const mutation = useMutation((item) => saveItem(item), {
    onSuccess: () => {
      formik.resetForm();
      return handleClose();
    },
  });

  const macList = ["", "MAC 1", "MAC 2"];

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: editItem ? editItem.id : "",
      name: editItem ? editItem.name : "",
      description: editItem ? editItem.description : "",
      macAddress: editItem ? editItem.macAddress : "",
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate(values);
    },
  });

  const handleBlur = (e) => {
    console.log("Blur:", e.target.value);
  };

  return (
    <div>
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Product</DialogTitle>
        <form onSubmit={formik.handleSubmit}>
          <DialogContent>
            {mutation.isError && (
              <DialogContentText color={"error"} fontSize={12}>
                {mutation.error.message}
              </DialogContentText>
            )}
            <Stack spacing={2}>
              <TextField
                fullWidth
                id="name"
                name="name"
                label="Name"
                value={formik.values.name}
                onChange={formik.handleChange}
                error={formik.touched.name && Boolean(formik.errors.name)}
                helperText={formik.touched.name && formik.errors.name}
              />
              <TextField
                fullWidth
                id="description"
                name="description"
                label="Description"
                type="text"
                multiline
                rows={4}
                value={formik.values.description}
                onChange={formik.handleChange}
                error={formik.touched.description && Boolean(formik.errors.description)}
                helperText={formik.touched.description && formik.errors.description}
              />
              <Autocomplete
                margin="normal"
                onBlur={handleBlur}
                options={macList}
                isOptionEqualToValue={(option, value) => option === value}
                sx={{ width: 300 }}
                getOptionLabel={(option) => option}
                id="macAddress"
                name="macAddress"
                onChange={(e, value) => {
                  formik.setFieldValue("macAddress", value);
                }}
                value={formik.values.macAddress}
                renderInput={(params) => (
                  <TextField
                    fullWidth
                    label="MacAddress"
                    {...params}
                    error={formik.touched.macAddress && Boolean(formik.errors.macAddress)}
                    helperText={formik.touched.macAddress && formik.errors.macAddress}
                  />
                )}
              />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleClose}>Cancel</Button>
            <Button color="primary" variant="contained" type="submit">
              {mutation.isLoading ? "Loading..." : "Save"}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </div>
  );
}
