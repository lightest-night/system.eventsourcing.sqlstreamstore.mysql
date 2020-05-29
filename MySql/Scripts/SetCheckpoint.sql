INSERT INTO checkpoints (checkpoint_id, checkpoint_id_hash, checkpoint)
VALUES (@CheckpointId, UNHEX(HEX(LOWER(@CheckpointId))), @Checkpoint)
ON DUPLICATE KEY UPDATE checkpoint = @Checkpoint;