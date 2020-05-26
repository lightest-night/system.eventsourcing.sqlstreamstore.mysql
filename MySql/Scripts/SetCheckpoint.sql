INSERT INTO checkpoints (checkpoint_id, checkpoint)
VALUES (@CheckpointId, @Checkpoint)
ON DUPLICATE KEY UPDATE checkpoint = @Checkpoint;