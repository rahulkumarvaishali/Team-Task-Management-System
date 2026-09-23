import api from "../api/axios";

export const getUsers = async () => {
  const response = await api.get("/Users");
  return response.data;
};

export const getUserById = async (id) => {
  const response = await api.get(`/Users/${id}`);
  return response.data;
};

export const updateUserRole = async (
  userId,
  role
) => {
  const response = await api.patch(
    `/Users/${userId}/role`,
    {
      role: Number(role),
    }
  );

  return response.data;
};