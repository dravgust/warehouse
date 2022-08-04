import * as React from "react";
import { useFormik } from "formik";
import { useMutation } from "react-query";
import * as yup from "yup";
import Stack from "@mui/material/Stack";
import { Icon, TextField, Box } from "@mui/material";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import DeletePromt from "./delete-promt";
import { deleteProduct, setProduct } from "services/warehouse-service";

const validationSchema = yup.object({
  name: yup
    .string("Enter product name")
    .min(3, "Name should be of minimum 3 characters length")
    .required("Name is required"),
  description: yup
    .string("Enter product description")
    .min(3, "Description should be of minimum 3 characters length"),
  metadata: yup.array().of(
    yup.object({
      value: yup.string().when(["isRequired"], {
        is: (isRequired) => isRequired,
        then: yup.string().required("Value is required"),
        otherwise: yup.string(),
      }),
    })
  ),
});

export default function ItemForm({ onSave = () => {}, onDelete = () => {}, item = {} }) {
  const mutation = useMutation(setProduct, {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const handleDelete = async (item) => {
    try {
      await deleteProduct(item);
      return onDelete();
    } catch (err) {
      console.log("delete-product", err);
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : "",
      name: item ? item.name : "",
      description: item ? item.description : "",
      metadata: item && item.metadata ? item.metadata : [],
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate(values);
    },
  });

  return (
    <Box
      component="form"
      onSubmit={formik.handleSubmit}
      sx={{
        "& .MuiTextField-root": { m: 1 },
      }}
      noValidate
      autoComplete="off"
    >
      {mutation.isError && (
        <SuiAlert style={{ fontSize: "12px" }} color={"error"} dismissible>
          {mutation.error.title || mutation.error.error}
        </SuiAlert>
      )}

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
        sx={{
          "& .MuiOutlinedInput-input": { width: "100%!important" },
        }}
        id="description"
        name="description"
        label="Description"
        type="text"
        multiline
        maxRows={10}
        value={formik.values.description}
        onChange={formik.handleChange}
        error={formik.touched.description && Boolean(formik.errors.description)}
        helperText={formik.touched.description && formik.errors.description}
      />

      {formik.values.metadata &&
        formik.values.metadata.map(({ key, value, type }, index) => (
          <TextField
            key={index}
            fullWidth
            label={key}
            id={`metadata[${index}].value`}
            name={`metadata[${index}].value`}
            value={value}
            type={type || "text"}
            InputLabelProps={{ shrink: true }}
            onChange={formik.handleChange}
            error={
              formik.touched.metadata &&
              formik.touched.metadata[index].value &&
              formik.errors.metadata &&
              formik.errors.metadata[index] &&
              Boolean(formik.errors.metadata[index].value)
            }
            helperText={
              formik.touched.metadata &&
              formik.touched.metadata[index].value &&
              formik.errors.metadata &&
              formik.errors.metadata[index] &&
              formik.errors.metadata[index].value
            }
          />
        ))}

      <Stack my={2} py={2} direction="row" spacing={1} justifyContent="end">
        <DeletePromt
          renderButton={(handleClickOpen) => (
            <SuiButton variant="text" color="error" onClick={handleClickOpen} disabled={!item.id}>
              <Icon>delete</Icon>&nbsp;delete
            </SuiButton>
          )}
          onDelete={() => handleDelete(item)}
        />
        <SuiButton color="success" variant="contained" type="submit">
          {mutation.isLoading ? (
            "Loading..."
          ) : (
            <>
              <Icon>save</Icon>&nbsp;save
            </>
          )}
        </SuiButton>
      </Stack>
    </Box>
  );
}
