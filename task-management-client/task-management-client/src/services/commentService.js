import api from "../api/axios";

export const getComments = async (
  taskId
) => {
  const response = await api.get(
    `/comments/${taskId}/comments`
  );

  return response.data;
};

export const addComment = async (
  taskId,
  content
) => {
  const response = await api.post(
    `/comments/${taskId}/comments`,
    {
      content,
    }
  );

  return response.data;
};

export const deleteComment = async (
  commentId
) => {
  const response = await api.delete(
    `/comments/${commentId}`
  );

  return response.data;
};