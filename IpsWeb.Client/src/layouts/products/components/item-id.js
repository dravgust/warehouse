import { useState } from "react";
import { useQuery } from "react-query";
import { useFormik } from "formik";
import {client} from 'utils/api-client'
import * as auth from 'auth-provider'

function ItemById() {
  const [id, setID] = useState("");
  const formik = useFormik({
    initialValues: {
      _id: "",
    },
    onSubmit: (values) => {
      console.log(JSON.stringify(values, null, 2));
      setID(values._id);
    },
  });
  const fetchItemById= async (id) => {
    const token = await auth.getToken();
    const res = await client(`items/${id}`, {token});
    return res;
  };
  const { data, error, isLoading } = useQuery(["item-id", id], () => fetchItemById(id));
  return (
    <div>
      <h1>Find by ID</h1>
      <form onSubmit={formik.handleSubmit}>
        <input
          id="_id"
          name="_id"
          type="text"
          onChange={formik.handleChange}
        ></input>
        <button type="submit">Find</button>
      </form>
      {error && <p>Error!</p>}
      {data && (
        <p>
          {data.data.name}, {data.data.description}
        </p>
      )}
      {isLoading && <p>Loading..</p>}
    </div>
  );
}

export default ItemById;